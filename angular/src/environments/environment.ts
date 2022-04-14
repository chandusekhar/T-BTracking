import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

export const environment = {
  production: false,
  application: {
    baseUrl,
    name: 'BugTracking',
    logoUrl: '',
  },
  oAuthConfig: {
    issuer: 'https://account.tpos.dev',
    redirectUri: baseUrl,
    clientId: 'BugTracking_App',
    responseType: 'code',
    scope: 'offline_access openid profile role email phone BugTracking',
  },
  apis: {
    default: {
      url: 'https://localhost:44348',
      rootNamespace: 'BugTracking',
    },
  },
  tdesk : {
    tdeskApi :  'https://tdesk.tpos.dev/',
    socketUrl : 'https://sio.tpos.dev/message',
    socketCallUrl: 'https://sio.tpos.dev/webrtc',
  }
} as Environment;
