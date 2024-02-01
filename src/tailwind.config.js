module.exports = {
    content: [
        "./Areas/**/*.{cs,cshtml,html,js}",
        "./Pages/**/*.{cs,cshtml,html,js}",
    ],
    theme: {
        extend: {},
    },
    variants: {
        extend: {},
    },
    plugins: [require('@tailwindcss/typography')],
}
