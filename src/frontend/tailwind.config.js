/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './apps/**/src/**/*.{html,ts}',
    './libs/**/src/**/*.{html,ts}',
  ],
  theme: {
    extend: {
      colors: {
        // Orange Car Rental Brand Colors
        primary: {
          50: '#fef6ee',
          100: '#fdebd6',
          200: '#fad3ac',
          300: '#f6b478',
          400: '#f28b41',
          500: '#ef6c1b',  // Main Orange Brand Color
          600: '#e05211',
          700: '#b93d10',
          800: '#933214',
          900: '#762b13',
          950: '#401308',
        },
        // Additional UI colors
        secondary: {
          50: '#f7f7f8',
          100: '#eeeef0',
          200: '#d9d9de',
          300: '#b7b8c1',
          400: '#90919f',
          500: '#727384',
          600: '#5c5d6d',
          700: '#4b4c58',
          800: '#40414b',
          900: '#383941',
          950: '#25252a',
        },
        success: {
          DEFAULT: '#10b981',
          light: '#d1fae5',
          dark: '#065f46',
        },
        warning: {
          DEFAULT: '#f59e0b',
          light: '#fef3c7',
          dark: '#92400e',
        },
        error: {
          DEFAULT: '#ef4444',
          light: '#fee2e2',
          dark: '#991b1b',
        },
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', '-apple-system', 'sans-serif'],
      },
      spacing: {
        '128': '32rem',
        '144': '36rem',
      },
      borderRadius: {
        '4xl': '2rem',
      },
    },
  },
  plugins: [],
};
