function callApi(endpoint, token) {

    const headers = new Headers();
    const bearer = `Bearer ${token}`;

    headers.append("Authorization", bearer);

    const options = {
        method: "GET",
        headers: headers
    };

    fetch(endpoint, options)
        .then(response => response.json())
        .then(response => {
            return response;
        }).catch(error => {
            console.error(error);
        });
}