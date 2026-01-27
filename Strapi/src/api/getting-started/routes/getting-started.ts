export default {
  routes: [
    {
      method: 'GET',
      path: '/getting-started',
      handler: 'getting-started.find',
      config: {
        auth: false,
      },
    },
    {
      method: 'PUT',
      path: '/getting-started',
      handler: 'getting-started.update',
    },
    {
      method: 'DELETE',
      path: '/getting-started',
      handler: 'getting-started.delete',
    },
  ],
};
