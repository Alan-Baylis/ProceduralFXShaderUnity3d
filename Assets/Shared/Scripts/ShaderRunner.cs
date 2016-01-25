﻿using UnityEngine;
using System.Collections;

public class ShaderRunner : MonoBehaviour 
{

	public Material material;
	public int width = 128;
	public int height = 128;
	public FilterMode mode;
	public RenderTexture targetTexture;
	public RenderTextureFormat format = RenderTextureFormat.ARGBFloat;
	public Shader inputTextureBlendShader;
	public int iterationsPerFrame = 1;

	private RenderTexture lastBuffer;
	private RenderTexture currentBuffer;
	private Material mat;
	private Texture2D inputTexture;
	private Material blendMaterial;
	private int lastTexId;

	public int IterationsPerFrame {
		set {
			iterationsPerFrame = value;
		}
		get {
			return iterationsPerFrame;
		}
	}

	public RenderTexture CurrentBuffer 
	{
		get 
		{
			return currentBuffer;
		}
	}

	public void Clear(){
		Graphics.SetRenderTarget( lastBuffer );
		GL.Clear(true,true,Color.black);
		Graphics.SetRenderTarget( currentBuffer );
		GL.Clear(true,true,Color.black);
	}

	void UseTexture(Texture2D texture){
		this.inputTexture = texture;
	}

	void ClearTexture(){
		inputTexture = null;
	}

	void Awake(){
		lastTexId = Shader.PropertyToID("_LastTex");

		mat = material;
		if( targetTexture != null ){
			currentBuffer = targetTexture;
			width = targetTexture.width;
			height = targetTexture.height;
			format = targetTexture.format;
		}
		else {
			currentBuffer = new RenderTexture( width,height,0,format );        
			currentBuffer.enableRandomWrite = true;
			currentBuffer.wrapMode = TextureWrapMode.Repeat;
		}
		lastBuffer = new RenderTexture( width,height,0,format );
		lastBuffer.enableRandomWrite = true;
		lastBuffer.filterMode = mode;
		currentBuffer.filterMode = mode;
		lastBuffer.wrapMode = currentBuffer.wrapMode;
		lastBuffer.Create();
		currentBuffer.Create();
		if( inputTextureBlendShader ){
			blendMaterial = new Material( inputTextureBlendShader );
		}

		Clear();
	}

	void Update () 
	{
		
		for(int i=iterationsPerFrame;i>0;i--){
			RenderTexture tmp = lastBuffer;
			lastBuffer = currentBuffer;
			currentBuffer = tmp;
			if( mat.HasProperty(lastTexId) ){
				mat.SetTexture(lastTexId,currentBuffer);
			}
			Graphics.Blit(lastBuffer,currentBuffer,mat);
			if( inputTexture != null ){
				Graphics.Blit(inputTexture, currentBuffer, blendMaterial );
			}

		}

	}
	
	
}
