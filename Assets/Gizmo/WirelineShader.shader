// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Gizmo/WirelineShader"
{
    //Properties to display within unity interface
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Cull off

        pass
        {
            CGPROGRAM //Start of CG language shader code

            #pragma vertex vertexFunction  
            #pragma geometry geometryFunction
            #pragma fragment fragmentFunction

            struct vertexFunctionInput
            {
                float4 position : POSITION;
            };

            struct vertexToGeometry
            {
                float4 position : POSITION;
            };

            struct geometryToFragment
            {
                float4	position : POSITION;
            };

            //Modifies vertices 
            //Processes one vertex at a time
            void vertexFunction(vertexFunctionInput input, out vertexToGeometry output)
            {
                output.position = UnityObjectToClipPos(input.position); //Convert vertices by model-view-projection matrix
            }

            //Has access to geometric primitives, depending on mesh topology (points, lines or triangles)
            //Processes one such primitive at a time
            //http://gamedev.stackexchange.com/questions/97009/geometry-shader-not-generating-geometry-for-some-vertices
            [maxvertexcount(2)]
            void geometryFunction(line vertexToGeometry input[2],
                inout LineStream<geometryToFragment> lineStream)
            {
                geometryToFragment gTF[2];
                gTF[0].position = input[0].position;
                gTF[1].position = input[1].position;
                //gTF[2].position = input[2].position;
                lineStream.Append(gTF[0]);
                lineStream.Append(gTF[1]);
                //lineStream.Append(gTF[2]);
                lineStream.RestartStrip(); //Not sure if needed
            }

            fixed4 _Color;
            //Computes colors for each pixel that is affected by the mesh / rasterization
            void fragmentFunction(geometryToFragment input, out float4 color:COLOR)
            {
                color = _Color;
            }

            ENDCG //End of CG language shader code
        }
    }
}