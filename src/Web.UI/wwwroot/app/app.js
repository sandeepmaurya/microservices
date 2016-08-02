(function () {
    angular.module('app', ["ngRoute"])
    .config(function ($routeProvider, $locationProvider) {
        $locationProvider.html5Mode({
            enabled: true,
            requireBase: false
        });

        $routeProvider.when("/store", { templateUrl: "/app/store/store.html" });
        $routeProvider.when("/product", { templateUrl: "/app/product/product.html" });
        $routeProvider.when("/cart", { templateUrl: "/app/order/cart.html" });
        $routeProvider.when("/order", { templateUrl: "/app/order/order.html" });
        $routeProvider.otherwise({ templateUrl: "/app/index.html" });
    });
})();