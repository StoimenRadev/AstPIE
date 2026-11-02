// @ts-check
import { defineConfig, envField } from 'astro/config';

import react from '@astrojs/react';

import node from '@astrojs/node';

// https://astro.build/config
export default defineConfig({
  output: 'server',

  integrations: [react()],

  adapter: node({
    mode: 'standalone'
  }),

  env: {
    schema: {
      // Address of the ASP.NET Core API. Set it in .env (see .env.example).
      API_URL: envField.string({ context: 'server', access: 'public', default: 'http://localhost:5080' })
    }
  }
});
