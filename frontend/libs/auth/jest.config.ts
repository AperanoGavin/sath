export default {
  displayName: 'auth',
  preset: '../../jest.preset.js',
  testEnvironment: 'node',
  transform: {
    '^.+\\.(ts|js)$': ['ts-jest', { tsconfig: '<rootDir>/tsconfig.spec.json' }],
  },
  moduleFileExtensions: ['ts', 'js', 'html'],
  moduleNameMapper: {
    '^@auth/(.*)$': '<rootDir>/src/lib/$1',
  },
  testMatch: ['**/+(*.)+(spec|test).+(ts|js)?(x)'],
  coverageDirectory: '../../coverage/libs/auth',
};
