window.thesisUpdatesEditor = {
    applyMarkdown: function (textarea, format) {
        if (!textarea) {
            return "";
        }

        textarea.focus();

        const current = textarea.value || "";
        const start = textarea.selectionStart ?? current.length;
        const end = textarea.selectionEnd ?? current.length;
        const selected = current.slice(start, end);
        const placeholder = selected || "text";

        let insertion = "";

        switch (format) {
            case "Heading":
                insertion = "## " + placeholder;
                break;
            case "Bold":
                insertion = "**" + placeholder + "**";
                break;
            case "Italic":
                insertion = "*" + placeholder + "*";
                break;
            case "UnorderedList":
                insertion = "- " + placeholder;
                break;
            case "OrderedList":
                insertion = "1. " + placeholder;
                break;
            case "Quote":
                insertion = "> " + placeholder;
                break;
            case "Code":
                insertion = "```\n" + (selected || "code") + "\n```";
                break;
            case "Link":
                insertion = "[" + placeholder + "](https://example.com)";
                break;
            default:
                insertion = placeholder;
                break;
        }

        const next = current.slice(0, start) + insertion + current.slice(end);
        textarea.value = next;

        const cursor = start + insertion.length;
        textarea.setSelectionRange(cursor, cursor);
        textarea.dispatchEvent(new Event("input", { bubbles: true }));

        return next;
    }
};
