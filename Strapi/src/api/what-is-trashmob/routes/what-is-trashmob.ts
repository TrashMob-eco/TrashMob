export default {
  routes: [
    {
      method: 'GET',
      path: '/what-is-trashmob',
      handler: 'what-is-trashmob.find',
      config: {
        auth: false,
      },
    },
    {
      method: 'PUT',
      path: '/what-is-trashmob',
      handler: 'what-is-trashmob.update',
    },
    {
      method: 'DELETE',
      path: '/what-is-trashmob',
      handler: 'what-is-trashmob.delete',
    },
  ],
};
