﻿using IIS.SLSharp.Annotations;

namespace IIS.SLSharp.Examples.Complex.Shaders
{
    public abstract class CubeShader : Shader
    {
        [Varying]
        private vec3 _uvw;

        [VertexIn]
        public vec4 Vertex;

        [FragmentOut]
        public vec4 Color;

        [Uniform]
        public abstract mat4 ModelViewProjectionMatrix { get; set; }

        // demonstrate using a shared library
        public SimplexNoiseShader Noise { get; private set; }

        [FragmentShader(true)]
        protected void FragmentMain()
        {
            var n = Noise.FBm(_uvw * 4.0f, 6, 1.6f, 0.7f);
            Color = new vec4(n);
        }

        [VertexShader(true)]
        public void VertexMain()
        {
            _uvw = (Vertex.xyz + new vec3(1.0f)) * 0.5f;
            gl_Position = ModelViewProjectionMatrix * Vertex;
        }

        // resource setup code
        protected CubeShader()
        {
            Noise = CreateSharedShader<SimplexNoiseShader>();
            Link(new[] { Noise });
        }

        public override void Dispose()
        {
            Noise.Dispose();
            base.Dispose();
        }

        public override void Begin()
        {
            Noise.BeginLibrary(this);
            base.Begin();
        }
    }
}