window.addEventListener('editor-markdown::setup', function(event)
{
    const mdIt = event.detail.markdownIt;
    // Footnote
    mdIt.use(window.markdownitFootnote);
    mdIt.renderer.rules.footnote_caption = (tokens, idx/*, options, env, slf*/) =>
    {
        var n = Number(tokens[idx].meta.id + 1).toString();
        if (tokens[idx].meta.subId > 0) { n += ':' + tokens[idx].meta.subId; }
        return n;
    };
    // Autolink
    mdIt.options.linkify = true;
});
