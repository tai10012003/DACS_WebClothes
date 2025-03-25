function thanhToan() {
    // Xử lý thanh toán
    alert("Xử lý thanh toán...");
}

function quayLai() {
    // Quay lại trang trước đó
    window.history.back();
    window.history.back();
}

document.getElementById('toggleNavButton').addEventListener('click', function() {
    var sidebar = document.getElementById('sidebar');
    var overlay = document.getElementById('overlay');
    sidebar.classList.toggle('show');
    overlay.classList.toggle('show');
});
document.addEventListener("DOMContentLoaded", function () {
    var carousel = document.getElementById("imageCarousel");
    setInterval(function () {
        document.querySelector('.carousel-control-next').click();
    }, 3000);
});
function goBackAndCloseSidebar() {
    window.history.back();
    closeSidebar();
  }
  
  function closeSidebar() {
    document.getElementById("sidebar").classList.remove("show");
    document.getElementById("overlay").classList.remove("show");
}

function googleTranslateElementInit() {
    new google.translate.TranslateElement(
        { pageLanguage: 'en' },
        'google_translate_element'
    );
}
function myFunction() {
    var element = document.body;
    element.classList.toggle("dark-mode");
    var htmlElement = document.getElementsByTagName("html")[0];
    htmlElement.classList.toggle("dark-mode");
}