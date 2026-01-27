export default {
  routes: [
    {
      method: 'GET',
      path: '/hero-section',
      handler: 'hero-section.find',
      config: {
        auth: false,
      },
    },
    {
      method: 'PUT',
      path: '/hero-section',
      handler: 'hero-section.update',
    },
    {
      method: 'DELETE',
      path: '/hero-section',
      handler: 'hero-section.delete',
    },
  ],
};
