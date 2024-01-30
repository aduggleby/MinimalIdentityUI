module.exports = {
    content: [
        "./Areas/**/*.{cshtml,html,js}",
        "./Pages/**/*.{cshtml,html,js}",
    ],
    theme: {
        extend: {},
    },
    variants: {
        extend: {},
    },
    plugins: [require('@tailwindcss/typography')],
}
