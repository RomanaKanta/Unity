Shader "Custom/Flat"
{
	Properties{
		_Color ("Main Color",Color)=(1,1,0,1)
	}
	SubShader{
		Pass{
			Color [_Color]
			Lighting Off
		}
	}
}