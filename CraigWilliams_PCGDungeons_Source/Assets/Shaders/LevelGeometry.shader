// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/LevelGeometry"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100
        CULL OFF
        Pass
        {
            Tags { "LightMode" = "ForwardBase" }

            CGPROGRAM
            #pragma vertex ProcessVertex
            #pragma fragment ProcessFragment
            #pragma target 3.0
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"	

            struct Vertex
            {
                float4 position : POSITION;
                fixed4 color : COLOR;
            };

            struct FragmentInput
            {
                float4 position : SV_POSITION;
                float4 worldPosition : TEXCOORD0;
                fixed4 color : TEXCOORD1;
            };

            // Vertex Shader
            FragmentInput ProcessVertex(Vertex vertex)
            {
                FragmentInput output;
                output.position = UnityObjectToClipPos(vertex.position);

                // Calculate world position for face-normal calculation in fragment shader.
                output.worldPosition = mul(unity_ObjectToWorld, vertex.position);

                // Gamma
                //output.color = pow((vertex.color + 0.055f) / 1.055f, 2.4f);
                output.color = vertex.color;
                return output;
            }

            // Fragment Shader
            fixed4 ProcessFragment(FragmentInput input) : SV_Target
            {
                // Compute the normal from the interpolated world position.
                half3 normal = normalize(cross(ddy(input.worldPosition),
                    ddx(input.worldPosition))); 
                
                // Calculate lighting and final color.
                half3 toCamera = normalize((_WorldSpaceCameraPos.xyz - input.worldPosition).xyz);
                half nl = saturate(dot(normal, toCamera));
                //return fixed4(nl, nl, nl, 1);
                return fixed4(input.color.rgb * (_LightColor0.rgb * nl), 1); // Simple lambert
            }
            ENDCG
        }
    }
}
