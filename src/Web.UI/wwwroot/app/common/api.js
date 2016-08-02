(function () {
    angular.module('app')
        .service('api', ['$location', main]);

    function main($location) {
        this.baseUrl = function () {
            return $location.protocol() + "://" + $location.host() + ":9090";
        }
    }
})();