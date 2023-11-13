using System;

namespace Zermos_Web.Utilities;

public static class FileSizeUtils
{
    public enum FileSizeUnit
    {
        Byte,
        Kilobyte,
        Megabyte,
        Gigabyte,
        Terabyte,
        Petabyte,
        Exabyte,
        best
    }
    
    public static string FormatFileSize(this int size, FileSizeUnit input, FileSizeUnit output = FileSizeUnit.best)
    {
        return FormatFileSize((long)size, input, output);
    }
    
    public static string FormatFileSize(this long size, FileSizeUnit input, FileSizeUnit output = FileSizeUnit.best)
    {
        if (output == FileSizeUnit.best)
        {
            if (size < 1024)
                output = FileSizeUnit.Byte;
            else if (size < 1024 * 1024)
                output = FileSizeUnit.Kilobyte;
            else if (size < 1024 * 1024 * 1024)
                output = FileSizeUnit.Megabyte;
            else if (size < 1024L * 1024 * 1024 * 1024)
                output = FileSizeUnit.Gigabyte;
            else if (size < 1024L * 1024 * 1024 * 1024 * 1024)
                output = FileSizeUnit.Terabyte;
            else if (size < 1024L * 1024 * 1024 * 1024 * 1024 * 1024)
                output = FileSizeUnit.Petabyte;
            else 
                output = FileSizeUnit.Exabyte;
        }

        if (input == output)
            return size + " " + output;

        double result = size;
        if (input < output)
        {
            for (int i = 0; i < (int)output - (int)input; i++)
                result /= 1024;
        }
        else
        {
            for (int i = 0; i < (int)input - (int)output; i++)
                result *= 1024;
        }

        return Math.Round(result, 2) + " " + output;
    }
}