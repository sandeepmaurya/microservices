(function () {
    angular.module('app')
        .controller('storectrl', ['$scope', '$http', 'api', '$rootScope', '$location', Main]);

    function Main($scope, $http, api, $rootScope, $location) {
        $http.get(api.baseUrl() + '/store/cities')
                .then(function (response) { $scope.cities = response.data; });

        $scope.$watch('selectedCity', function (city) {
            if (city) {
                $http.get(api.baseUrl() + '/store/cities/' + city.Id + '/stores')
                .then(function (response) { $scope.localities = response.data; });
            }
        });

        $scope.buildOrder = function () {
            $rootScope.cityId = $scope.selectedCity.Id;
            $rootScope.storeId = $scope.selectedLocality.StoreId;
            $location.path('/product');
        }
    }
})();