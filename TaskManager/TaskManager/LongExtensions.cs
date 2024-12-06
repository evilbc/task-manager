using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager
{
	public static class LongExtensions
	{
		// https://github.com/microsoft/TVHelpers/blob/master/csharp/MediaAppSample/MediaAppSample.Core/Extensions/LongExtensions.cs
		public static string ToStringAsMemory(this long sizeInBytes)
		{
			if (sizeInBytes > 1024 * 1024 * 1024)
				return $"{((sizeInBytes / 1024.0) / 1024.0 / 1024):N2} GB";
			else if (sizeInBytes > 1024 * 1024)
				return $"{((sizeInBytes / 1024.0) / 1024.0):N2} MB";
			else if (sizeInBytes > 1024)
				return $"{(sizeInBytes / 1024.0):N2} KB";
			return $"{sizeInBytes} bytes";
		}
	}
}
