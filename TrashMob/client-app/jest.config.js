/** @type {import('ts-jest').JestConfigWithTsJest} **/
module.exports = {
  preset: "ts-jest",
  testEnvironment: "jsdom",
  // Use mock implementations instead.
  moduleNameMapper: {
    "react-azure-maps": "<rootDir>/tests/mock/react-azure-maps.js",
    "react-phone-input-2": "<rootDir>/tests/mock/react-phone-input-2.js",
    "react-simple-captcha": "<rootDir>/tests/mock/react-simple-captcha.js"
  },
  transform: {
    "^.+.tsx?$": ["ts-jest",{}],
    ".+\\.(svg|css|styl|less|sass|scss|png|jpg|ttf|woff|woff2)$": "jest-transform-stub"
  },
  setupFiles: [
    "<rootDir>/tests/setup.js"
  ],
  setupFilesAfterEnv: ["@testing-library/jest-dom/extend-expect"]

};
