/*
 * This is a .NET port of the Douglas Crockford's JSMin 'C' project.
 * The author's copyright message is reproduced below.
 */

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;


public sealed class JsMinifier
{
    const double AVERAGE_COMPRESSION_RATIO = 0.6;
    const int EOF = -1;
    private StringReader _reader;
    private StringWriter _writer;
    private int _theA, _theB, _lookAhead = EOF, _theX = EOF, _theY = EOF;
    private readonly object _minificationSynchronizer = new object();

    public static int GetEstimatedOutputLength(string content) => (int) Math.Floor(content.Length * AVERAGE_COMPRESSION_RATIO);

    public string Minify(string content)
    {
        if (content == null || string.IsNullOrWhiteSpace(content)) return string.Empty;
        var outputBuilder = new StringBuilder(content.Length);
        MinifyInternal(content, outputBuilder);
        return outputBuilder.ToString();
    }

    public void Minify(string content, StringBuilder outputBuilder)
    {
        if (content == null || outputBuilder == null || string.IsNullOrWhiteSpace(content)) return;
        MinifyInternal(content, outputBuilder);
    }

    private void MinifyInternal(string content, StringBuilder outputBuilder)
    {
        lock (_minificationSynchronizer)
        {
            _theA = 0;
            _theB = 0;
            _lookAhead = EOF;
            _theX = EOF;
            _theY = EOF;
            _reader = new StringReader(content);
            _writer = new StringWriter(outputBuilder);

            try
            {
                StartMinification();
                _writer.Flush();
                outputBuilder.TrimStart();
            }
            catch (JsMinificationException)
            {
                outputBuilder.Clear();
                throw;
            }
            finally
            {
                _reader.Dispose();
                _reader = null;
                _writer.Dispose();
                _writer = null;
            }
        }
    }

    private static bool IsAlphanum(int codeunit) => (codeunit >= 'a' && codeunit <= 'z') ||
                                                    (codeunit >= '0' && codeunit <= '9') ||
                                                    (codeunit >= 'A' && codeunit <= 'Z') || codeunit == '_' ||
                                                    codeunit == '$' || codeunit == '\\' || codeunit > 126;

    private int Get()
    {
        int codeunit = _lookAhead;
        _lookAhead = EOF;
        if (codeunit == EOF) codeunit = _reader.Read();
        if (codeunit >= ' ' || codeunit == '\n' || codeunit == EOF) return codeunit;
        return codeunit == '\r' ? '\n' : ' ';
    }

    private int Peek() => _lookAhead = Get();

    //updated to support /**/ comments too
    private int Next()
    {
        int codeunit = Get();
        if (codeunit == '/')
        {
            switch (Peek())
            {
                case '/':
                    // Single-line comment
                    while ((codeunit = Get()) != '\n' && codeunit != EOF)
                    {
                    }
                    break;
                case '*':
                    // Multi-line comment
                    Get(); // Consume the '*'
                    while (true)
                    {
                        codeunit = Get();
                        if (codeunit == EOF)
                            throw new JsMinificationException("Unterminated multi-line comment.");
                        if (codeunit == '*' && Peek() == '/')
                        {
                            Get(); // Consume the '/'
                            codeunit = ' '; // Replace comment with a space
                            break;
                        }
                    }
                    break;
            }
        }

        _theY = _theX;
        _theX = codeunit;
        return codeunit;
    }

    private void Action(int determined)
    {
        if (determined == 1)
        {
            Put(_theA);
            if ((_theY == '\n' || _theY == ' ') && (_theA == '+' || _theA == '-' || _theA == '*' || _theA == '/') &&
                (_theB == '+' || _theB == '-' || _theB == '*' || _theB == '/')) Put(_theY);
        }

        if (determined <= 2)
        {
            _theA = _theB;
            if (_theA == '\'' || _theA == '"' || _theA == '`')
            {
                for (;;)
                {
                    Put(_theA);
                    _theA = Get();
                    if (_theA == _theB) break;
                    if (_theA == '\\')
                    {
                        Put(_theA);
                        _theA = Get();
                    }

                    if (_theA == EOF) throw new JsMinificationException("Unterminated string literal.");
                }
            }
        }

        if (determined <= 3)
        {
            _theB = Next();
            if (_theB == '/' && (_theA == '(' || _theA == ',' || _theA == '=' || _theA == ':' || _theA == '[' ||
                                 _theA == '!' || _theA == '&' || _theA == '|' || _theA == '?' || _theA == '+' ||
                                 _theA == '-' || _theA == '~' || _theA == '*' || _theA == '/' || _theA == '{' ||
                                 _theA == '}' || _theA == ';'))
            {
                Put(_theA);
                if (_theA == '/' || _theA == '*') Put(' ');
                Put(_theB);
                for (;;)
                {
                    _theA = Get();
                    if (_theA == '[')
                    {
                        for (;;)
                        {
                            Put(_theA);
                            _theA = Get();
                            if (_theA == ']') break;
                            if (_theA == '\\')
                            {
                                Put(_theA);
                                _theA = Get();
                            }

                            if (_theA == EOF)
                                throw new JsMinificationException("Unterminated set in Regular Expression literal.");
                        }
                    }
                    else if (_theA == '/')
                    {
                        switch (Peek())
                        {
                            case '/':
                            case '*':
                                throw new JsMinificationException("Unterminated set in Regular Expression literal.");
                        }

                        break;
                    }
                    else if (_theA == '\\')
                    {
                        Put(_theA);
                        _theA = Get();
                    }

                    if (_theA == EOF) throw new JsMinificationException("Unterminated Regular Expression literal.");
                    Put(_theA);
                }

                _theB = Next();
            }
        }
    }

    private void StartMinification()
    {
        if (Peek() == 0xEF)
        {
            Get();
            Get();
            Get();
        }

        _theA = '\n';
        Action(3);
        while (_theA != EOF)
        {
            switch (_theA)
            {
                case ' ':
                    Action(IsAlphanum(_theB) ? 1 : 2);
                    break;
                case '\n':
                    switch (_theB)
                    {
                        case '{':
                        case '[':
                        case '(':
                        case '+':
                        case '-':
                        case '!':
                        case '~':
                            Action(1);
                            break;
                        case ' ':
                            Action(3);
                            break;
                        default:
                            Action(IsAlphanum(_theB) ? 1 : 2);
                            break;
                    }

                    break;
                default:
                    switch (_theB)
                    {
                        case ' ':
                            Action(IsAlphanum(_theA) ? 1 : 3);
                            break;
                        case '\n':
                            switch (_theA)
                            {
                                case '}':
                                case ']':
                                case ')':
                                case '+':
                                case '-':
                                case '"':
                                case '\'':
                                case '`':
                                    Action(1);
                                    break;
                                default:
                                    Action(IsAlphanum(_theA) ? 1 : 3);
                                    break;
                            }

                            break;
                        default:
                            Action(1);
                            break;
                    }

                    break;
            }
        }
    }

    private void Put(int c) => _writer.Write((char) c);

#if !NETSTANDARD1_0
    [Serializable]
#endif
    public sealed class JsMinificationException : Exception
    {
        public JsMinificationException(string message) : base(message)
        {
        }

        public JsMinificationException(string message, Exception innerException) : base(message, innerException)
        {
        }
#if !NETSTANDARD1_0
        private JsMinificationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
#endif
    }
}


/// <summary>
/// Extensions for Char
/// </summary>
public static class CharExtensions
{
    [MethodImpl((MethodImplOptions) 256 /* AggressiveInlining */)]
    public static bool IsWhitespace(this char source)
    {
        return source == ' ' || (source >= '\t' && source <= '\r');
    }
}


public static class StringBuilderExtensions
{
    public static StringBuilder TrimStart(this StringBuilder source)
    {
        int charCount = source.Length;
        if (charCount == 0)
        {
            return source;
        }

        int charIndex = 0;

        while (charIndex < charCount)
        {
            char charValue = source[charIndex];
            if (!charValue.IsWhitespace())
            {
                break;
            }

            charIndex++;
        }

        if (charIndex > 0)
        {
            source.Remove(0, charIndex);
        }

        return source;
    }
}