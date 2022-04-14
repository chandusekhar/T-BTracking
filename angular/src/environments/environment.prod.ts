import { Environment } from '@abp/ng.core';

const baseUrl = 'https://t-btracking.tpos.dev';

export const environment = {
  production: true,
  application: {
    baseUrl,
    name: 'BugTracking',
    logoUrl: '',
  },
  oAuthConfig: {
    issuer: 'https://account.tpos.dev',
    redirectUri: baseUrl,
    clientId: 'BugTracking',
    responseType: 'code',
    scope: 'offline_access BugTracking',
  },
  apis: {
    default: {
      url: 'https://t-btracking.tpos.dev',
      rootNamespace: 'BugTracking',
    },
  },
  tdesk : {
    tdeskApi :  'https://tdesk.tpos.dev/',
    socketUrl : 'http://sio.tpos.dev/message',
    socketCallUrl: 'http://sio.tpos.dev/webrtc',
  }
} as Environment;
