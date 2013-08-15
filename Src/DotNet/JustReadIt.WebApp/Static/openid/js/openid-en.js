var providers_large = {
  google : {
    name : 'Google',
    url : 'https://www.google.com/accounts/o8/id'
  },
  facebook : {
    name : 'Facebook',
    url : 'http://www.facebook.com/'
  },
  twitter : {
    name : 'Twitter',
    url : 'http://twitter.com/'
  },
  liveid : {
    name : 'LiveID',
    url : 'https://live.com/'
  },
  yahoo : {
    name : 'Yahoo',
    url : 'http://me.yahoo.com/'
  },
  myopenid : {
    name : 'MyOpenID',
    label : 'Enter your MyOpenID username:',
    url : 'http://{username}.myopenid.com/'
  }
/* aol : {
    name : 'AOL',
    label : 'Enter your AOL screenname:',
    url : 'http://openid.aol.com/{username}'
  }, */
};

var providers_small = {
  openid : {
    name : 'OpenID',
    label : 'Enter your OpenID:',
    url : null
  },
  livejournal : {
    name : 'LiveJournal',
    label : 'Enter your Livejournal username:',
    url : 'http://{username}.livejournal.com/'
  },
  /* flickr: {
    name: 'Flickr',        
    label: 'Enter your Flickr username:',
    url: 'http://flickr.com/{username}/'
  }, */
  /* technorati: {
    name: 'Technorati',
    label: 'Enter your Technorati username:',
    url: 'http://technorati.com/people/technorati/{username}/'
  }, */
  wordpress : {
    name : 'Wordpress',
    label : 'Enter your Wordpress username:',
    url : 'http://{username}.wordpress.com/'
  },
  blogger : {
    name : 'Blogger',
    label : 'Enter your Blogger account:',
    url : 'http://{username}.blogspot.com/'
  },
  verisign : {
    name : 'Verisign',
    label : 'Enter your Verisign username:',
    url : 'http://{username}.pip.verisignlabs.com/'
  },
  /* vidoop: {
    name: 'Vidoop',
    label: 'Enter your Vidoop username:',
    url: 'http://{username}.myvidoop.com/'
  }, */
  /* launchpad: {
    name: 'Launchpad',
    label: 'Enter your Launchpad username:',
    url: 'https://launchpad.net/~{username}'
  }, */
  claimid : {
    name : 'ClaimID',
    label : 'Enter your ClaimID username:',
    url : 'http://claimid.com/{username}'
  },
  clickpass : {
    name : 'ClickPass',
    label : 'Enter your ClickPass username:',
    url : 'http://clickpass.com/public/{username}'
  },
  google_profile : {
    name : 'Google Profile',
    label : 'Enter your Google Profile username:',
    url : 'http://www.google.com/profiles/{username}'
  }
};

openid.locale = 'en';
openid.sprite = 'en'; // reused in german& japan localization
openid.demo_text = 'In client demo mode. Normally would have submitted OpenID:';
openid.signin_text = 'Sign In';
openid.image_title = 'Sign in with {provider}';
