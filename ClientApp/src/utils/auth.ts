
let authenticated = false;
export default {
  authenticated() {
    return  window.sessionStorage.getItem('authenticated') == '1' ? true : false
    //return authenticated;
  },
  getToken() {
    return window.sessionStorage.getItem('apiToken')
  },
  logIn(token: string) {
    window.sessionStorage.setItem('authenticated', '1')
    authenticated = true;
    window.sessionStorage.setItem('apiToken', token);
  },
  logOut() { 
    window.sessionStorage.setItem('authenticated', '0')
    authenticated = false;
    window.sessionStorage.setItem('apiToken', '');
  }
};
