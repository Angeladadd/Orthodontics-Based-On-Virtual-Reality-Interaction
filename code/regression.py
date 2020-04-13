import numpy as np
import matplotlib.pyplot as plt
from sklearn.preprocessing import PolynomialFeatures
from sklearn.preprocessing import StandardScaler
from sklearn.pipeline import Pipeline
from sklearn.linear_model import LinearRegression

#x = np.random.uniform(-3,3,size=100)
x = [28.21383,24.97599,22.24316,19.48654,16.55384,11.52745,3.937866,-5.458901,-13.05779,-17.77903,-21.18414,-22.97624,-25.03778]
_x = np.array(x)
X = _x.reshape(-1,1)
y = [-5.921997,2.197391,10.14865,16.80978,23.62298,27.64277,29.66666,29.2431,26.3411,17.45458,10.6098,2.48847,-7.177981]
_y = np.array(y)
z = [12.41128,10.29781,9.965306,9.847241,8.669186,8.231961,7.747763,8.481681,8.894978,9.513444,9.474886,10.43088,12.05847]
_z = np.array(z)
#y = 0.5 * x**2 + x + 2 +np.random.normal(0,1,100)

poly_reg = Pipeline([
    ('poly',PolynomialFeatures(degree=6)),
    ('std_scaler',StandardScaler()),
    ('lin_reg',LinearRegression())
    ])

poly_reg2 = Pipeline([
    ('poly',PolynomialFeatures(degree=4)),
    ('std_scaler',StandardScaler()),
    ('lin_reg',LinearRegression())
    ])

#poly_reg = PolynomialFeatures(degree = 1)
#X = poly_reg.fit_transform(_x)
#line_reg = LinearRegression()
#line_reg.fit(X,y)

test_x = [31.45167]
_test_x = np.array(test_x)
Test_x = _test_x.reshape(-1,1)

test_x11 = [-13.05779-(-13.05779+17.77903)/2]
_test_x11 = np.array(test_x11)
Test_x11 = _test_x11.reshape(-1,1)

test_x16 = [-25.03778+(-25.03778+22.97624)]
_test_x16 = np.array(test_x16)
Test_x16 = _test_x16.reshape(-1,1)

poly_reg.fit(X,_y)
poly_reg2.fit(X,_z)
y_predict = poly_reg.predict(X)
z_predict = poly_reg2.predict(X)


#poly_reg2.fit(X,_z)
plt.scatter(_x,_y)
plt.plot(np.sort(_x),y_predict[np.argsort(_x)],color='r')
plt.show()
plt.scatter(_x,_z)
plt.plot(np.sort(_x),z_predict[np.argsort(_x)],color='r')
plt.show()


newx = [-24.63067,-22.25235,-19.86638,-16.50715,-11.62734,-7.530358,-2.595384,2.621914,7.52895,12.38255,17.99037,20.40489,23.3968,24.77657]
_newx = np.array(newx)
newX = _newx.reshape(-1,1)

newy = [-9.728711,0.8148623,9.321835,15.9585,21.28661,25.32502,26.56643,26.57108,25.36898,21.93693,17.34018,10.30516,2.206023,-8.529264]
_newy = np.array(newy)
#newY = _newy.reshape(-1,1)

newz = [4.638245,3.162342,2.614676,1.502662,3.457983,3.672607,4.283814,4.269794,4.231588,3.845889,2.221299,2.953367,3.368798,4.411522]
_newz = np.array(newz)
#newZ = _newz.reshape(-1,1)

poly_reg3 = Pipeline([
    ('poly',PolynomialFeatures(degree=6)),
    ('std_scaler',StandardScaler()),
    ('lin_reg',LinearRegression())
    ])

poly_reg4 = Pipeline([
    ('poly',PolynomialFeatures(degree=4)),
    ('std_scaler',StandardScaler()),
    ('lin_reg',LinearRegression())
    ])

poly_reg3.fit(newX,_newy)
poly_reg4.fit(newX,_newz)
newy_predict = poly_reg3.predict(newX)
newz_predict = poly_reg4.predict(newX)

plt.scatter(_newx,_newy)
plt.plot(np.sort(_newx),newy_predict[np.argsort(_newx)],color='r')
plt.show()
plt.scatter(_newx,_newz)
plt.plot(np.sort(_newx),newz_predict[np.argsort(_newx)],color='r')
plt.show()

test_x17 = [-24.63067+(-24.63067+22.25235)]
_test_x17 = np.array(test_x17)
Test_x17 = _test_x17.reshape(-1,1)

test_x32 = [24.77657+(24.77657-23.3968)]
_test_x32 = np.array(test_x32)
Test_x32 = _test_x32.reshape(-1,1)


print test_x
print poly_reg.predict(Test_x)
print poly_reg2.predict(Test_x)

print test_x11
print poly_reg.predict(Test_x11)
print poly_reg2.predict(Test_x11)

print test_x16
print poly_reg.predict(Test_x16)
print poly_reg2.predict(Test_x16)

print test_x17
print poly_reg.predict(Test_x17)
print poly_reg4.predict(Test_x17)

print test_x32
print poly_reg3.predict(Test_x32)
print poly_reg4.predict(Test_x32)
