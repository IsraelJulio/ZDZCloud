export default defineNuxtConfig({
  modules: ["@nuxt/ui"],
  css: ["~/assets/css/main.css"],
  devtools: { enabled: true },
  fonts: {
    providers: {
      google: false,
      googleicons: false,
      fontsource: false,
      fontshare: false,
      bunny: false,
    },
  },
  icon: {
    serverBundle: {
      collections: ["lucide"],
    },
  },
  runtimeConfig: {
    public: {
      // apiBase: 'http://localhost:5257'
      apiBase: "http://zdzcloud-production.up.railway.app",
    },
  },
});
