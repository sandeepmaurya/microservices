(function () {
    angular.module('app')
        .controller('productctrl', ['$scope', '$routeParams', '$http', 'api', '$rootScope', '$location',
            function ($scope, $routeParams, $http, api, $rootScope, $location) {
                $scope.storeId = $rootScope.storeId;
                LoadMenu($scope, $http, api);

                $scope.addToCart = function (product) {
                    $rootScope.$broadcast('addToCart', { Product: product, Quantity: 1 });
                }

                function LoadMenu($scope, $http, api) {
                    $http.get(api.baseUrl() + '/product/stores/' + $rootScope.storeId + '/products')
                            .then(function (response) {
                                $scope.menu = [];
                                for (i = 0; i < response.data.length; i++) {
                                    var item = response.data[i];
                                    $scope.menu.push({
                                        Id: item.Product.Id,
                                        Name: item.Product.Name,
                                        Price: item.Price,
                                        ImgUri: item.Product.ImageUri
                                    });
                                }
                            });
                }
            }]);
})();