import js from '@eslint/js';
import tseslint from 'typescript-eslint';
import reactPlugin from 'eslint-plugin-react';
import reactHooksPlugin from 'eslint-plugin-react-hooks';
import jsxA11yPlugin from 'eslint-plugin-jsx-a11y';
import simpleImportSort from 'eslint-plugin-simple-import-sort';
import prettierConfig from 'eslint-config-prettier';
import globals from 'globals';

export default tseslint.config(
    // Global ignores (replaces .eslintignore)
    {
        ignores: [
            'node_modules/**',
            'public/**',
            'coverage/**',
            'build/**',
            'dist/**',
            'vite.config.ts',
            'tailwind.config.js',
            'postcss.config.js',
            'e2e/**',
            'playwright.config.ts',
            'playwright-report/**',
        ],
    },

    // Base recommended configs
    js.configs.recommended,
    ...tseslint.configs.recommended,

    // TypeScript files configuration
    {
        files: ['**/*.{ts,tsx}'],
        languageOptions: {
            globals: {
                ...globals.browser,
                ...globals.es2021,
            },
            parserOptions: {
                projectService: true,
                tsconfigRootDir: import.meta.dirname,
                ecmaFeatures: { jsx: true },
            },
        },
        plugins: {
            react: reactPlugin,
            'react-hooks': reactHooksPlugin,
            'jsx-a11y': jsxA11yPlugin,
            'simple-import-sort': simpleImportSort,
        },
        settings: {
            react: { version: 'detect' },
        },
        rules: {
            // Disable rules that were off in legacy config
            'no-use-before-define': 'off',
            'no-unused-vars': 'off',
            'no-undef': 'off',
            'no-prototype-builtins': 'off',
            'no-extra-boolean-cast': 'off',
            'no-empty': 'off',
            'no-inner-declarations': 'off',
            'no-irregular-whitespace': 'off',
            'no-console': 'off',
            'no-shadow': 'off',
            'no-alert': 'off',
            'no-param-reassign': 'off',
            'no-nested-ternary': 'off',
            'no-restricted-globals': 'off',
            'consistent-return': 'off',
            'default-case': 'off',
            'prefer-const': 'off',
            'prefer-destructuring': 'off',
            'class-methods-use-this': 'off',
            'no-plusplus': 'off',
            'no-await-in-loop': 'off',
            'no-restricted-syntax': 'off',
            camelcase: 'off',
            eqeqeq: 'off',
            radix: 'off',

            // TypeScript rules
            '@typescript-eslint/no-explicit-any': 'off',
            '@typescript-eslint/no-unused-vars': 'off',
            '@typescript-eslint/ban-ts-comment': 'off',
            '@typescript-eslint/no-this-alias': 'off',
            '@typescript-eslint/no-empty-object-type': 'off',

            // React rules
            'react/react-in-jsx-scope': 'off',
            'react/prop-types': 'off',
            'react/no-unescaped-entities': 'off',
            'react/no-unknown-property': 'off',
            'react/jsx-no-bind': 'off',
            'react/jsx-props-no-spreading': 'off',
            'react/jsx-no-useless-fragment': 'off',
            'react/no-unstable-nested-components': 'off',
            'react/jsx-no-constructed-context-values': 'off',
            'react/no-unused-prop-types': 'off',
            'react/destructuring-assignment': 'off',
            'react/no-array-index-key': 'off',
            'react/no-danger': 'off',
            'react/require-default-props': 'off',
            'react/button-has-type': 'off',
            'react/hook-use-state': 'off',
            'react/iframe-missing-sandbox': 'off',
            'react/function-component-definition': 'off',

            // React hooks
            'react-hooks/exhaustive-deps': 'off',

            // JSX a11y warnings (the only enforced rules)
            'jsx-a11y/no-noninteractive-element-to-interactive-role': 'warn',
            'jsx-a11y/click-events-have-key-events': 'warn',
            'jsx-a11y/label-has-associated-control': [
                'warn',
                {
                    controlComponents: ['Input', 'Textarea', 'Select', 'Checkbox', 'SelectTrigger'],
                    depth: 3,
                },
            ],
            'jsx-a11y/no-static-element-interactions': 'warn',
            'jsx-a11y/control-has-associated-label': 'warn',
        },
    },

    // Prettier must be last to disable conflicting rules
    prettierConfig,
);
