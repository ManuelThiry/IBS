document.addEventListener("DOMContentLoaded", function() {
    const teamButton = document.querySelector('.teams');
    const engageButton = document.querySelector('.engage');

    // Fonction pour gérer l'animation et la redirection
    function handleButtonClick(button) {
        button.addEventListener('click', function(event) {
            event.preventDefault(); // Empêche la redirection immédiate

            // Appliquer l'animation
            button.classList.add('fade-out');

            // Attendre la fin de l'animation avant de rediriger
            setTimeout(function() {
                window.location.href = button.parentElement.getAttribute('href');
            }, 500); // Durée de l'animation en millisecondes
        });
    }

    // Appliquer la fonction à chaque bouton
    handleButtonClick(teamButton);
    handleButtonClick(engageButton);
});
