using System;
using System.Numerics;

namespace Web.Utility
{
  public class Common
  {
    internal static Vector3 CalculateRegion(int i)
    {
      // From SO, https://stackoverflow.com/a/41141648
      int index = i + 1;
      // wth
      int s = (int)Math.Ceiling(Math.Sqrt(index)) + (int)((Math.Ceiling(Math.Sqrt(index)) % 2 + 1) % 2);
      int ringIndex = 0;
      int p = 1;
      if (s > 1)
      {
        ringIndex = i - (s - 2) * (s - 2);
        p = s * s - (s - 2) * (s - 2);
      }

      int ri = (ringIndex + s / 2) % p;

      int x = 0;
      if (s > 1)
      {
        if (ri < p / 4) x = ri;
        else
        {
          if (ri <= p / 4 * 2 - 1) x = p / 4;
          else
          {
            if (ri <= p / 4 * 3) x = p / 4 * 3 - ri;
            else x = 0;
          }
        }
      }

      int z = 0;
      if (s > 1)
        z = ri < p / 4 ? 0 :
                ri <= p / 4 * 2 - 1 ? ri - p / 4 :
                        ri <= p / 4 * 3 ? p / 4 :
                                p - ri;

      x -= s / 2;
      z -= s / 2;

      return new Vector3(x, 0, z);
    }
  }
}
