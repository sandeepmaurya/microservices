(function () {
    angular.module('app')
        .controller('orderctrl', ['$scope', '$routeParams', '$http', 'api', '$rootScope', '$location', '$window',
            function ($scope, $routeParams, $http, api, $rootScope, $location, $window) {
                $scope.order = $rootScope.order;
                updateGross();

                if (!$scope.order.Customer) {
                    $scope.order.Customer = {};
                }

                if (!$scope.order.Address) {
                    $scope.order.Address = {
                        City: $scope.order.Store.City,
                        State: $scope.order.Store.State
                    };
                }

                $scope.pay = function () {
                    var customerDetails = {
                    };

                    $http({
                        method: 'POST',
                        url: api.baseUrl() + '/order/' + $scope.order.Id + '/customer',
                        data: $scope.order,
                        headers: { 'Content-Type': 'application/json' }
                    }).success(function (data, status, headers, config) {
                        $scope.order = data;
                        $window.location.href = '/dummygateway?amt=' + encodeURIComponent($scope.order.TotalAmount) +
                        '&cb=' + encodeURIComponent('/payment/complete?oid=' + encodeURIComponent($scope.order.Id));
                    });
                }

                $scope.applyCoupon = function () {
                    $http({
                        method: 'POST',
                        url: api.baseUrl() + '/order/' + $scope.order.Id + '/coupon',
                        data: $scope.couponCode,
                        headers: { 'Content-Type': 'application/json' }
                    }).success(function (data, status, headers, config) {
                        $scope.order = data;
                        updateGross();
                    }).error(function (data, status, headers, config) {
                        alert('Invalid Coupon!');
                    });
                }

                $scope.paymentDisabled = function () {
                    if (!$scope.order ||
                        !$scope.order.Customer ||
                        !$scope.order.Customer.FirstName ||
                        !$scope.order.Customer.LastName ||
                        !$scope.order.Customer.Email ||
                        !$scope.order.Customer.Mobile) {
                        return true;
                    }

                    if (!$scope.order ||
                        !$scope.order.Address ||
                        !$scope.order.Address.AddressLine1 ||
                        !$scope.order.Address.AddressLine2) {
                        return true;
                    }

                    return false;
                }

                function updateGross() {
                    var gross = 0.0;
                    for (i = 0; i < $scope.order.Items.length; i++) {
                        gross += $scope.order.Items[i].Amount;
                    }
                    $scope.order.GrossAmount = gross;
                }
            }]);

})();