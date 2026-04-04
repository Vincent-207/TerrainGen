Shader "Terrain/Derivative"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _BaseTexture("Base Texture", 2D) = "white" {}
        _BaseFrequency("Base Frequency", Range(0.0, 64.0)) = 4.0
        _BaseAmplitude("Base Amplitude", Range(0.0, 16.0)) = 1.0
        _Scale("Scale", Float) = 0.5
        _Octaves("Octaves", Range(0, 16)) = 8
        _Lacunarity("Lacunarity", Range(0.0, 4.0)) = 2.0
        _Gain("Gain", Range(0.0, 4.0)) = 0.5
        [Vector2] _Offset("Offset ", Vector) = (23.2, 15.4, 0, 0)
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }

        Pass
        {
            Tags
            {
                "LightMode" = "SRPDefaultUnlit"
            }

            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _BaseTexture_ST;
                float _BaseFrequency;
                float _BaseAmplitude;
                float _Scale;
                int _Octaves;
                float _Lacunarity;
                float _Gain;
                float2 _Offset;
            CBUFFER_END

            TEXTURE2D(_BaseTexture);
            TEXTURE2D(_NoiseTexture);
            SAMPLER(sampler_NoiseTexture);
            SAMPLER(sampler_BaseTexture);
            void Unity_GradientNoise_float(float2 UV, float Scale, out float Out);
            float2 unity_gradientNoise_dir(float2 p);
            float unity_gradientNoise( float2 p );
            float FractalBrownianMotion(float2 uv);
            float4 noised(float3 x);
            float hash13(float3 p3);
            float terrain( float2 p );
            float hash12(float2 p);

            const float3x3 m3  = float3x3( 0.00,  0.80,  0.60,
                      -0.80,  0.36, -0.48,
                      -0.60, -0.48,  0.64 );
            const float3x3 m3i = float3x3( 0.00, -0.80, -0.60,
                       0.80,  0.36, -0.48,
                       0.60, -0.48,  0.64 );
            struct appdata
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };


            v2f vert(appdata v)
            {
                v2f o = (v2f)0;
                
                float3 positionWS = TransformObjectToWorld(v.positionOS.xyz);
                float2 positionWS2D = float2(positionWS.x, positionWS.z);
                
                // float elevation = FractalBrownianMotion(v.uv);
                float elevation = terrain(v.uv + _Offset);
                // float elevation = v.uv.x;
                positionWS.y = elevation;

                o.positionCS = TransformWorldToHClip(positionWS);
                o.uv = TRANSFORM_TEX(v.uv, _BaseTexture);

                return o;
            }

            // value noise, and its analytical derivatives
            // wadwa
            float3 noisedOther( float2 x )
            {
                float2 f = frac(x);
                // #if USE_SMOOTH_NOISE==0
                // float2 u = f*f*(3.0-2.0*f);
                // float2 du = 6.0*f*(1.0-f);
                // #else
                float2 u = f*f*f*(f*(f*6.0-15.0)+10.0);
                float2 du = 30.0*f*f*(f*(f-2.0)+1.0);
                // #endif

                    // texture version    
                float2 p = floor(x);
                
                // float a = SAMPLE_TEXTURE2D(_NoiseTexture, sampler_NoiseTexture,(p+float2(0.5,0.5))/256.0 ).x;
                // float b = SAMPLE_TEXTURE2D(_NoiseTexture, sampler_NoiseTexture,(p+float2(1.5,0.5))/256.0 ).x;
                // float c = SAMPLE_TEXTURE2D(_NoiseTexture, sampler_NoiseTexture,(p+float2(0.5,1.5))/256.0 ).x;
                // float d = SAMPLE_TEXTURE2D(_NoiseTexture, sampler_NoiseTexture,(p+float2(1.5,1.5))/256.0 ).x;
                // float a = textureLod( iChannel0, (p+float2(0.5,0.5))/256.0, 0.0 ).x;
                // float b = textureLod( iChannel0, (p+float2(1.5,0.5))/256.0, 0.0 ).x;
                // float c = textureLod( iChannel0, (p+float2(0.5,1.5))/256.0, 0.0 ).x;
                // float d = textureLod( iChannel0, (p+float2(1.5,1.5))/256.0, 0.0 ).x;
                float a = hash12(p + float2(0.5,0.5));
                float b = hash12(p + float2(1.5,0.5));
                float c = hash12(p + float2(0.5,1.5));
                float d = hash12(p + float2(1.5,1.5));
                return float3(a+(b-a)*u.x+(c-a)*u.y+(a-b-c+d)*u.x*u.y,
				du*(float2(b-a,c-a)+(a-b-c+d)*u.yx));
            }

            float FractalBrownianMotion(float2 uv)
            {
                float sum = 0;
                float freq = _BaseFrequency, amp = _BaseAmplitude;
                for(int i = 0; i < _Octaves; i++)
                {
                    float noise =1.0;
                    // Unity_GradientNoise_float(uv / freq, _Scale,  noise);
                    sum += noise * amp;
                    freq *= _Lacunarity;
                    amp *= _Gain;
                }

                return sum;
            }

            float4 frag(v2f i) : SV_TARGET
            {
                float4 textureColor = SAMPLE_TEXTURE2D(_BaseTexture, sampler_BaseTexture, i.uv);
                return textureColor * _BaseColor;
            }

            const float2x2 m = float2x2(0.8,-0.6,0.6,0.8);

            float terrain( float2 p )
            {
                float a = 0.0;
                float b = _BaseAmplitude;
                float2  d = float2(0.0,0.0);
                for( int i=0; i<_Octaves; i++ )
                {
                    float3 n = noisedOther(p);
                    d += n.yz;
                    a += b*n.x/(1.0+dot(d,d));
                    b *= _Gain;
                    p *= _Lacunarity;
                }
                return a;
            }

            const float2x2 m2 = float2x2(0.8,-0.6,0.6,0.8);
            const float SC = 250.0;

            float terrainH( float2 x )
            {
                float2  p = x*0.003/SC;
                float a = 0.0;
                float b = 1.0;
                float2  d = float2(0.0, 0.0);
                for( int i=0; i<16; i++ )
                {
                    float3 n = noisedOther(p);
                    d += n.yz;
                    a += b*n.x/(1.0+dot(d,d));
                    b *= 0.5;
                    p = mul(m2, p)*2.0;
                }

                // #if USE_SMOOTH_NOISE==1
                a *= 0.9;
                // #endif
                return SC*120.0*a;
            }

            // returns 3D fbm and its 3 derivatives
            // ERROR type mismatch with for d +=. not sure why. 
            /* float4 fbm( float3 x, int octaves )
            {
                float f = 1.98;  // could be 2.0
                float s = 0.49;  // could be 0.5
                float a = 0.0;
                float b = 0.5;
                float3  d = float3(0.0,0.0,0.0);
                float3x3  m = float3x3(
                            1.0,0.0,0.0,
                            0.0,1.0,0.0,
                            0.0,0.0,1.0);
                for( int i=0; i<octaves; i++ )
                {
                    float4 n = noised(x);
                    a += b * n.x;          // accumulate values
                    d += b * m * n.yzw;      // accumulate derivatives
                    b *= s;
                    x = f*m3*x;
                    m = f*m3i*m;
                }
                return float4( a, d );
            } */

            float4 noised(float3 x)
            {
                float3 p = floor(x);
                float3 w = frac(x);

                float3 u = w*w*w*(w*(w*6.0 - 15.0)+10.0);
                float3 du = 30.0*w*2*(w*(1-1.0)+2.0);

                float a = hash13(p+float3(0,0,0));
                float b = hash13(p+float3(1,0,0));
                float c = hash13(p+float3(0,1,0));
                float d = hash13(p+float3(1,1,0));
                float e = hash13(p+float3(0,0,1));
                float f = hash13(p+float3(1,0,1));
                float g = hash13(p+float3(0,1,1));
                float h = hash13(p+float3(1,1,1));

                float k0 = a;
                float k1 = b - a;
                float k2 = c - a;
                float k3 = e - a;
                float k4 = a - b - c + d;
                float k5 = a - c - e + g;
                float k6 = a - b - e + f;
                float k7 = - a + b + c - d + e - f - g + h;

                return float4( -1.0+2.0*(k0 + k1*u.x + k2*u.y + k3*u.z + k4*u.x*u.y + k5*u.y*u.z + k6*u.z*u.x + k7*u.x*u.y*u.z),
                 2.0*du *float3( k1 + k4*u.y + k6*u.z + k7*u.y*u.z,
                               k2 + k5*u.z + k4*u.x + k7*u.z*u.x,
                               k3 + k6*u.x + k5*u.y + k7*u.x*u.y ) );
            }
            float hash12(float2 p)
            {
                float3 p3  = frac(float3(p.xyx) * .1031);
                p3 += dot(p3, p3.yzx + 33.33);
                return frac((p3.x + p3.y) * p3.z);
            }
            float hash13(float3 p3)
            {
                p3  = frac(p3 * .1031);
                p3 += dot(p3, p3.yzx + 33.33);
                return frac((p3.x + p3.y) * p3.z);
            }

            float hash11(float p)
            {
                p = frac(p * .1031);
                p *= p + 33.33;
                p *= p + p;
                return frac(p);
            }
            float3 hash33(float3 p3)
            {
                p3 = frac(p3 * float3(.1031, .1030, .0973));
                p3 += dot(p3, p3.yxz+33.33);
                return frac((p3.xxy + p3.yxx)*p3.zyx);
            }

            ENDHLSL
        }

        // DepthOnly and DepthNormals passes added in Part 4.

        Pass
        {
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            ZWrite On
            ColorMask R

            HLSLPROGRAM
            #pragma vertex depthOnlyVert
            #pragma fragment depthOnlyFrag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 positionOS : POSITION;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
            };

            v2f depthOnlyVert(appdata v)
            {
                v2f o = (v2f)0;

                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);

                return o;
            }

            float depthOnlyFrag(v2f i) : SV_TARGET
            {
                return i.positionCS.z;
            }

            ENDHLSL
        }

        Pass
        {
            Tags
            {
                "LightMode" = "DepthNormals"
            }

            ZWrite On

            HLSLPROGRAM
            #pragma vertex depthNormalsVert
            #pragma fragment depthNormalsFrag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
            };

            v2f depthNormalsVert(appdata v)
            {
                v2f o = (v2f)0;

                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.normalWS = NormalizeNormalPerVertex(normalWS);

                return o;
            }

            float4 depthNormalsFrag(v2f i) : SV_TARGET
            {
                float3 normalWS = NormalizeNormalPerPixel(i.normalWS);
                return float4(normalWS, 0.0f);
            }

            ENDHLSL
        }
    }
}
