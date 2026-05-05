window._cyA11yConfig = {
    primaryColor: "#0f6faa",
    position: {
        mobile: "bottom-right",
        desktop: "bottom-right"
    },
    margins: {
        desktop: {
            top: 24,
            bottom: 24,
            left: 24,
            right: 24
        },
        mobile: {
            top: 16,
            bottom: 16,
            left: 16,
            right: 16
        }
    },
    keyboard: {
        enabled: true,
        shortcut: "alt+a"
    },
    language: {
        default: (document.documentElement.lang || "en").split("-")[0].toLowerCase()
    },
    modules: {
        statement: {
            enabled: false,
            displayInWidget: false,
            url: ""
        }
    }
};

window._cyA11yAssets = {
    fonts: "/vendor/accessibility-widget/fonts/"
};
