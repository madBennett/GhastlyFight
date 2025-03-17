//Madison Bennett
//CS 596

Shader "Unlit/Swirl Shaders"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FadeTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            static const float PI = 3.14159265f;

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _FadeTex;
            float4 _FadeTex_ST;

            fixed2 Swirl(fixed2 uv)
            {
                //center of the texture
                fixed2 center = (0.5,0.5);
                //offset by center
                float2 delta = uv - center;
                //conversion to polar coords
                float theta = atan2(delta.y,delta.x);
                float r = length(delta);
                //intensity of swirl
                float k = 1.5;
                //time varible for speed of swirl
                float time = k*(sin(_Time.y+PI/2));
                //calc overall angle
                float angle = theta + r*time*2*PI; //where r will determine how much the angle will need to be altered
                //conversion to rectangle coords
                uv.x = r*cos(angle);
                uv.y = r*sin(angle);
                //shift tiling back to correct possition
                uv += center;

                return uv;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //apply swirl
                i.uv = Swirl(i.uv);
                // sample the texture
                fixed4 col1 = tex2D(_MainTex, i.uv);//Fade inbetween each image
                fixed4 col2 = tex2D(_FadeTex, i.uv);

                //calc time
                float time = 0.5*(sin(_Time.y)+1);

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

                //return the inbetween of the textures based on time
                return lerp(col1, col2, time);
            }
            ENDCG
        }
    }
}
