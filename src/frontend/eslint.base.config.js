// @ts-check
/**
 * Shared ESLint configuration for all frontend applications.
 * Apps extend this config and can override rules as needed.
 */
const eslint = require("@eslint/js");
const { defineConfig } = require("eslint/config");
const tseslint = require("typescript-eslint");
const angular = require("angular-eslint");

/**
 * Creates the base ESLint configuration for Angular apps.
 * @param {Object} options - Configuration options
 * @param {string} [options.prefix='app'] - The selector prefix for components/directives
 * @returns ESLint flat config array
 */
function createBaseConfig(options = {}) {
  const prefix = options.prefix || 'app';

  return defineConfig([
    {
      files: ["**/*.ts"],
      extends: [
        eslint.configs.recommended,
        tseslint.configs.recommended,
        tseslint.configs.stylistic,
        angular.configs.tsRecommended,
      ],
      processor: angular.processInlineTemplates,
      rules: {
        "@angular-eslint/directive-selector": [
          "error",
          {
            type: "attribute",
            prefix: prefix,
            style: "camelCase",
          },
        ],
        "@angular-eslint/component-selector": [
          "error",
          {
            type: "element",
            prefix: prefix,
            style: "kebab-case",
          },
        ],
        "@typescript-eslint/no-unused-vars": [
          "error",
          {
            argsIgnorePattern: "^_",
            varsIgnorePattern: "^_",
          },
        ],
        "@angular-eslint/prefer-inject": "off",
        // Single-line if statements without brackets when possible
        // "multi-or-nest" requires braces if body spans multiple lines, otherwise no braces
        // Set to "warn" as auto-fix doesn't format ideally - manual review preferred
        "curly": ["warn", "multi-or-nest", "consistent"],
      },
    },
    {
      files: ["**/*.html"],
      extends: [
        angular.configs.templateRecommended,
        angular.configs.templateAccessibility,
      ],
      rules: {
        "@angular-eslint/template/label-has-associated-control": "off",
      },
    }
  ]);
}

module.exports = { createBaseConfig };
