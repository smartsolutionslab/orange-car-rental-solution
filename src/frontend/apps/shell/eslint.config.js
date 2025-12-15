// @ts-check
/**
 * ESLint configuration for shell app.
 * Extends shared base configuration.
 */
const { createBaseConfig } = require("../../eslint.base.config");

module.exports = createBaseConfig({ prefix: 'app' });
