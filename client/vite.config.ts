import { defineConfig} from 'vite';

export default defineConfig({
    server: {
      host: 'localhost',
      port: 4200,
      hmr: {
        protocol: 'ws',
        host: 'localhost',
        port: 4200,
      }
    }
  });