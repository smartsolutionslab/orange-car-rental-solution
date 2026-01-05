import type { StorybookConfig } from '@storybook/angular';
import { dirname, join, resolve } from 'path';

const config: StorybookConfig = {
  stories: [
    '../libs/**/*.stories.@(js|jsx|mjs|ts|tsx)',
  ],
  addons: [
    '@storybook/addon-essentials',
  ],
  framework: {
    name: '@storybook/angular',
    options: {},
  },
  staticDirs: ['../apps/public-portal/src/assets'],
  core: {
    disableTelemetry: true,
  },
  // Override webpack to fix Tailwind CSS 4 compatibility
  webpackFinal: async (config) => {
    // Find and modify PostCSS loader to use @tailwindcss/postcss instead of tailwindcss
    if (config.module?.rules) {
      config.module.rules = config.module.rules.map((rule) => {
        if (rule && typeof rule === 'object' && 'use' in rule) {
          const useArray = Array.isArray(rule.use) ? rule.use : [rule.use];
          rule.use = useArray.map((use) => {
            if (typeof use === 'object' && use !== null && 'loader' in use) {
              const loader = use as { loader?: string; options?: Record<string, unknown> };
              if (loader.loader?.includes('postcss-loader')) {
                // Override postcss plugins to use @tailwindcss/postcss
                loader.options = {
                  ...loader.options,
                  postcssOptions: {
                    plugins: [
                      ['@tailwindcss/postcss', {}],
                    ],
                  },
                };
              }
            }
            return use;
          });
        }
        return rule;
      });
    }
    return config;
  },
};

export default config;
