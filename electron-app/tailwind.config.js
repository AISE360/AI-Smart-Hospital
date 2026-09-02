/** @type {import('tailwindcss').Config} */
export default {
  content: ["./index.html", "./src/**/*.{js,ts,jsx,tsx}"],
  theme: {
    extend: {
      fontFamily: { sans: ['Inter','ui-sans-serif','system-ui'] },
      colors: {
        primary: { 50:'#eff6ff',100:'#dbeafe',400:'#60a5fa',500:'#2563eb',600:'#1d4ed8',700:'#1e40af',800:'#1e3a8a' },
        clinical: { red:'#dc2626', amber:'#d97706', green:'#059669', slate:'#334155' }
      },
      boxShadow: {
        'soft': '0 2px 10px rgba(0,0,0,0.06)',
        'card': '0 4px 24px rgba(0,0,0,0.07)',
        'glow': '0 8px 32px rgba(37,99,235,0.18)',
      },
      animation: {
        'fade-in': 'fadeIn 0.5s ease-out',
        'slide-up': 'slideUp 0.4s ease-out',
      },
      keyframes: {
        fadeIn: { '0%':{opacity:0}, '100%':{opacity:1} },
        slideUp: { '0%':{opacity:0, transform:'translateY(8px)'}, '100%':{opacity:1, transform:'translateY(0)'} },
      }
    },
  },
  plugins: [],
}
