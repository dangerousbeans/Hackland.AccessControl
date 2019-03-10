var app = angular.module("app", []);
app.controller("PasswordController", function ($scope) {
    
});
app.directive('passwordField', function () {
    return {
        restrict: 'E',
        require: 'ngModel',
        template:
            '<style type="text/css">' +
            '   .wide-popover { width: 300px; }' +
            '</style>' +
            '<div class="input-group">' +
            '   <input type="password" ng-model="ngModel" class="form-control" required="required" placeholder="{{ placeholder }}" style="-webkit-text-security:disc;text-security:disc;moz-text-security:disc;-mox-text-security:disc;" />' +
            '   <div class="input-group-append" style="cursor: pointer">'+
            '       <span class="input-group-text" ng-mouseover="showPopOver = true" ng-mouseleave="showPopOver = false" uib-popover-html="popoverHtml" popover-placement="right" popover-trigger="none" popover-title="Password rules" popover-is-open="showPopOver" popover-class="wide-popover" popover-enable="percent < 100">' +
            '         {{ percent < 40 ? "Weak" : percent < 60 ? "Fair" : percent < 100 ? "Good" : percent >= 100 ? "Strong" : "" }} ' +
            '         <i class="fa fa-info-circle" ng-if="percent < 100" style="margin-left:5px;"></i>' +
            '         <i class="fa fa-check" ng-if="percent >= 100" style="margin-left:5px;"></i>' +
            '       </span>' +
            '   </div>' +
            '</div>' +
            '<div class="progress" style="height: 5px" ng-if="percent > 0">' +
            '   <div class="progress-bar" role="progressbar" aria-valuenow="{{ percent }}" aria-valuemin="0" aria-valuemax="100" style="width: {{percent}}%" ng-class="{\'progress-bar-danger\': percent <= 40, \'progress-bar-warning\': percent > 40 && percent < 100, \'progress-bar-info\': percent >= 100 }">' +
            '       <span class="sr-only">{{ percent }} Complete</span>' +
            '   </div>' +
            '</div>',
        scope: {
            ngModel: '=',
            placeholder: '=?'
        },
        controller: function ($scope) {
            $scope.placeholder = $scope.placeholder || 'Password';
            $scope.percent = 0;
            $scope.showPopOver = false;

            $scope.$watch('ngModel', function (value) {
                var rules = {
                    length: undefined,
                    hasDigit: undefined,
                    hasSymbol: undefined,
                    hasLowerCase: undefined,
                    hasUpperCase: undefined,
                };

                if (value) {
                    rules.length = value.length >= 6;
                    rules.hasLowerCase = !!value.match(/[a-z]/);
                    rules.hasUpperCase = !!value.match(/[A-Z]/);
                    rules.hasDigit = !!value.match(/[0-9]/);
                    rules.hasSymbol = !!value.match(/[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/);
                }

                var values = _.values(rules);
                var completedRules = _.filter(values, function (v) { return !!v; });
                $scope.percent = Math.floor((completedRules.length / values.length) * 100);
                $scope.rules = rules;

                var popoverHtml = '<ul class="list-unstyled">';
                if (!rules.length) {
                    popoverHtml += '    <li>Minimum 6 characters</li>';
                }
                if (!rules.hasLowerCase) {
                    popoverHtml += '    <li>Need at least one lowercase letter</li>';
                }
                if (!rules.hasUpperCase) {
                    popoverHtml += '    <li>Need at least one uppercase letter</li>';
                }
                if (!rules.hasDigit) {
                    popoverHtml += '    <li>Need at least one digit</li>';
                }
                if (!rules.hasSymbol) {
                    popoverHtml += '    <li>Need at least one symbol</li>';
                }
                popoverHtml += '</ul>';
                $scope.popoverHtml = popoverHtml;
            });
        }
    };
});