

// Function to handle a failed request
function RequestFailed(result) {
    // Display a popup with the result's "text" property
    alert("Request Failed: " + result.text);
}

// Function to handle a successful request
function RequestSuccesful(result) {
    // Display a popup with the result's "text" property
    alert("Request Successful: " + result.text);
}
