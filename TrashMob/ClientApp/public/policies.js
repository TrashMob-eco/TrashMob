/**
 * Enter here the user flows and custom policies for your B2C application
 * To learn more about user flows, visit: https://docs.microsoft.com/en-us/azure/active-directory-b2c/user-flow-overview
 * To learn more about custom policies, visit: https://docs.microsoft.com/en-us/azure/active-directory-b2c/custom-policy-overview
 */
const b2cPolicies = {
    names: {
        signUpSignIn: "b2c_1_signupsignin1",
        forgotPassword: "b2c_1_passwordreset1",
        editProfile: "b2c_1_profileediting1"
    },
    authorities: {
        signUpSignIn: {
            authority: "https://trashmob.b2clogin.com/trashmob.onmicrosoft.com/b2c_1_signupsignin1",
        },
        forgotPassword: {
            authority: "https://trashmob.b2clogin.com/trashmob.onmicrosoft.com/b2c_1_passwordreset",
        },
        editProfile: {
            authority: "https://trashmob.b2clogin.com/trashmob.onmicrosoft.com/b2c_1_profileediting1"
        }
    },
    authorityDomain: "trashmob.b2clogin.com"
}