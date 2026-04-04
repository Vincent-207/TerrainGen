Shader "Terrain/Derivative"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _BaseTexture("Base Texture", 2D) = "white" {}
        _BaseFrequency("Base Frequency", Range(0.0, 64.0)) = 4.0
        _BaseAmplitude("Base Amplitude", Range(0.0, 128.0)) = 1.5
        _Scale("Scale", Float) = 0.5
        _Octaves("Octaves", Range(0, 16)) = 8
        _Lacunarity("Lacunarity", Float) = 2.0
        _Gain("Gain", Float) = 0.5
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
            CBUFFER_END

            TEXTURE2D(_BaseTexture);
            SAMPLER(sampler_BaseTexture);
            
            float terrain( float2 p );
            float3 noisedOther( float2 x );

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
                float elevation = terrain(v.uv);
                // float elevation = v.uv.x;
                positionWS.y = elevation;

                o.positionCS = TransformWorldToHClip(positionWS);
                o.uv = TRANSFORM_TEX(v.uv, _BaseTexture);

                return o;
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
                float b = 1.0;
                float2  d = float2(0.0,0.0);
                for( int i=0; i<15; i++ )
                {
                    float3 n = noisedOther(p);
                    d += n.yz;
                    a += b*n.x/(1.0+dot(d,d));
                    b *= 0.5;
                    p = mul(m, p)*2.0;
                }
                return a;
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
            float3 noisedOther( float2 x )
            {
                float2 f = frac(x);
                // #if USE_SMOOTH_NOISE==0
                // float2 u = f*f*(3.0-2.0*f);
                // float2 du = 6.0*f*(1.0-f);
                // #else
                float2 u = f*f*f*(f*(f*6.0-15.0)+10.0);
                float2 du = 30.0*f*f*(f*(f-2.0)+1.0);
                float2 p = floor(x);
                float a = hash12(p + float2(0.0,0.0));
                float b = hash12(p + float2(1.0,0.0));
                float c = hash12(p + float2(0.0,1.0));
                float d = hash12(p + float2(1.0,1.0));
                return float3(a+(b-a)*u.x+(c-a)*u.y+(a-b-c+d)*u.x*u.y,
				du*(float2(b-a,c-a)+(a-b-c+d)*u.yx));
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
