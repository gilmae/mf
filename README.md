# mf

mfDotnet is a library for scanning html documents and extracting [Microformat2](https://microformats.org/wiki/microformats2) documents.

## Usage

mfDotnet can be used to parse html strings:

```
string html = @"<article class=\"h-entry\"><h1 class=\"p-name\">Hello</h1></article>";
Parser p = new Parser();
Uri uri = new Uri("https://example.org");
var doc = p.Parse(html, uri);
```

Or loaded from a web page:

```
Parser p = new Parser();
Uri uri = new Uri("https://microformats.org");
var doc = p.Parse(uri);
```

## TODO

* Backcompatible formats
* Loading from url

