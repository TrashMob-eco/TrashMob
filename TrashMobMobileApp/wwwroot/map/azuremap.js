function InitMap() {
    var map = new atlas.Map('tmMap', {
        view: "Auto",
        center: [-122.33, 47.6],
        zoom: 12,
        language: 'en-US',

        authOptions: {
            authType: 'subscriptionKey',
            subscriptionKey: '5p5HTkSxyEJQS3Jo5n6uVbdtY_zmhItA4QpxWaQh0x8'
        }
    });
}
