// @ts-check
/**
 * ESLint configuration for public-portal app.
 * Extends shared base configuration.
 */
const { createBaseConfig } = require("../../eslint.base.config");

module.exports = createBaseConfig({ prefix: 'app' });
