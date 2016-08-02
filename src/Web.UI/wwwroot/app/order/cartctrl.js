(function () {
    angular.module('app')
        .controller('cartctrl', ['$scope', '$routeParams', '$http', 'api', '$rootScope', '$location',
            function ($scope, $routeParams, $http, api, $rootScope, $location) {
                $scope.totalItems = 0;

                $scope.$on('addToCart', function (event, arg) {
                    if (!$rootScope.orderId) {
                        createNewOrder($rootScope.cityId, $rootScope.storeId, function () {
                            addItem(arg.Product, arg.Quantity);
                        });
                    }
                    else {
                        addItem(arg.Product, arg.Quantity);
                    }
                });

                loadOrder();

                $scope.checkout = function () {
                    $http({
                        method: 'POST',
                        url: api.baseUrl() + '/order/' + $scope.order.Id + '/checkout',
                        headers: { 'Content-Type': 'application/json' }
                    }).success(function (data, status, headers, config) {
                        $scope.order = data;
                        $rootScope.order = $scope.order;
                        $location.path('/order');
                    });
                }

                $scope.viewCart = function () {
                    $location.path('/cart');
                }

                $scope.continueShopping = function () {
                    $location.path('/product');
                }

                $scope.removeItem = function (item) {
                    $http({
                        method: 'DELETE',
                        url: api.baseUrl() + '/order/' + $scope.order.Id + '/items/' + item.Id,
                    }).success(function (data, status, headers, config) {
                        $scope.order = data;
                        updateTotalItems();
                    });
                }

                function createNewOrder(cityId, storeId, callback) {
                    $http({
                        method: 'POST',
                        url: api.baseUrl() + '/order?cityId=' + cityId + '&storeId=' + storeId,
                        headers: { 'Content-Type': 'application/json' }
                    }).success(function (data, status, headers, config) {
                        $rootScope.orderId = data.Id;
                        $scope.order = data;
                        callback();
                    });
                }

                function addItem(product, quantity) {
                    $http({
                        method: 'POST',
                        url: api.baseUrl() + '/order/' + $scope.order.Id + '/items',
                        data: {
                            ProductId: product.Id,
                            Quantity: quantity,
                        },
                        headers: { 'Content-Type': 'application/json' }
                    }).success(function (data, status, headers, config) {
                        $scope.order = data;
                        updateTotalItems();
                    });
                }

                function loadOrder() {
                    if ($rootScope.orderId) {
                        $http({
                            method: 'GET',
                            url: api.baseUrl() + '/order/' + $rootScope.orderId + '?/' + Math.random()
                        }).success(function (data, status, headers, config) {
                            $scope.order = data;
                            updateTotalItems();
                        });
                    }
                }

                function updateTotalItems() {
                    if ($scope.order && $scope.order.Items) {
                        $scope.totalItems = 0;
                        for (i = 0; i < $scope.order.Items.length; i++) {
                            $scope.totalItems += $scope.order.Items[i].Quantity;
                        }
                    }
                }

            }]);

})();