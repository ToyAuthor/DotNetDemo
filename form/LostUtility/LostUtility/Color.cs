
namespace LostUtility
{
	public class Color
	{
		// 實際值域還是 0~255
		public float R = 0;
		public float G = 0;
		public float B = 0;

		public Color( float r, float g, float b )
		{
			R = r;
			G = g;
			B = b;
		}

		public Color()
		{
			R = 0;
			G = 0;
			B = 0;
		}
	}
}
