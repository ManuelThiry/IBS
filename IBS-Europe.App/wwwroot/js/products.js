
function showProduct(id) {
    document.cookie = `selectedProduct=${id}; path=/; max-age=2592000;`;
    window.location.href = "/Products";
    
}

