// @ts-check
/**
 * ESLint configuration for call-center-portal app.
 * Extends shared base configuration.
 */
const { createBaseConfig } = require("../../eslint.base.config");

module.exports = createBaseConfig({ prefix: 'app' });
