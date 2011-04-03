﻿using System;
using System.Reflection;
using IIS.SLSharp.Annotations;

namespace IIS.SLSharp.Examples.Simple.Shaders
{
    public abstract class MyShader : Shader
    {
        // expose a simple uniform (this can be directly accessed from client code)
        [Uniform]
        public abstract float Blue { set; get; }

        [Varying]
        private vec2 _uv;

        [VertexIn]
        public vec4 Vertex;

        [FragmentOut]
        public vec4 Color;

        // demonstrate using a shared library
        public InvertShader Invert { get; private set; }

        [FragmentShader(true)]
        protected void FragmentMain()
        {
            Color = Invert.Invert(new vec4(_uv, Blue, 1.0f));
        }

        [VertexShader(true)]
        public void VertexMain()
        {
            _uv = (Vertex.xy + new vec2(1.0f)) * 0.5f;
            gl_Position = Vertex;
        }

        // resource setup code
        protected MyShader()
        {
            Invert = CreateSharedShader<InvertShader>();
            Link(new[] { Invert });
        }

        public override void Dispose()
        {
            Invert.Dispose();
            base.Dispose();
        }

        public override void Begin()
        {
            Invert.BeginLibrary(this);
            base.Begin();
        }

        public void RenderQuad()
        {
            RenderQuad(() => Vertex);
        }
    }
}