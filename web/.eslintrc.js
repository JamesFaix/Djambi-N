module.exports = {
    "env": {
        "browser": true,
        "es6": true
    },
    "extends": [
        "plugin:react/recommended",
        "plugin:@typescript-eslint/recommended"
    ],
    "globals": {
        "Atomics": "readonly",
        "SharedArrayBuffer": "readonly"
    },
    "parser": "@typescript-eslint/parser",
    "parserOptions": {
        "ecmaFeatures": {
            "jsx": true
        },
        "ecmaVersion": 2018,
        "sourceType": "module"
    },
    "plugins": [
        "react",
        "@typescript-eslint"
    ],

    //See rule defaults:
    //Standard: https://eslint.org/docs/rules/
    //TypeScript: https://github.com/typescript-eslint/typescript-eslint/tree/master/packages/eslint-plugin
    "rules": {
        "curly": "warn",
        "default-case": "error",
        "dot-notation": "warn",
        // "no-alert": "warn",
        "no-caller": "warn",
        // "no-empty-function": "warn", //Off because this flags empty constructors with property parameters
        "no-eval": "warn",
        "no-extend-native": "error",
        "no-extra-bind": "warn",
        // "no-extra-parens": "warn", //Off because this flags parens around typescript-specific expressions
        "no-floating-decimal": "warn",
        "no-implicit-coercion": "error",
        "no-invalid-this": "error",
        "no-iterator": "error",
        "no-labels": "error",
        "no-lone-blocks": "warn",
        "no-octal-escape": "warn",
        "no-proto": "warn",
        "no-prototype-builtins": "warn",
        "no-return-assign": "error",
        "no-return-await": "warn",
        "no-self-compare": "error",
        "no-sequences": "error",
        "no-template-curly-in-string": "error",
        "no-undef-init": "warn",
        "no-unused-expressions": "warn",
        "no-use-before-define": "error",
        "no-useless-call": "warn",
        "no-useless-catch": "error",
        "no-useless-concat": "warn",
        "no-useless-return": "warn",
        "no-void": "error",
        // "no-warning-comments": "warn",
        "no-with": "warn",
        "require-await": "error",
        "semi": "warn",
        "yoda": "warn",

        "@typescript-eslint/explicit-member-accessibility": "off",
        "@typescript-eslint/explicit-function-return-type": [
            "off",
            {
                allowExpressions: true,
                allowTypedFunctionExpressions: true
            }
        ],
        "@typescript-eslint/member-delimiter-style" : [
            "warn",
            {
                multiline: {
                    delimiter: "comma",
                    requireLast: false
                },
                singleline: {
                    delimiter: "comma",
                    requireLast: false
                }
            }
        ],
        "@typescript-eslint/no-explicit-any": "off",
        "@typescript-eslint/no-extraneous-class": "warn",
        "@typescript-eslint/no-for-in-array": "error",
        "@typescript-eslint/no-parameter-properties": "off",
        "@typescript-eslint/no-this-alias": "error",
        "@typescript-eslint/no-unused-vars": [
            "off",
            {
                argsIgnorePattern: "_",
                varsIgnorePattern: "_"
            }
        ],
        "@typescript-eslint/no-useless-constructor": "warn",
        "@typescript-eslint/type-annotation-spacing": [
            "warn",
            {
                before: true,
                after: true
            }
        ],
        "@typescript-eslint/unified-signatures": "warn"
    },
    settings:  {
        react:  {
          version:  'detect',
        },
      },
};