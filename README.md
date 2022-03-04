# ZackURPPostProcess

# 1.全息扫描 <br>
简单的全息扫描后处理，分为球体，圆柱体和立方体(坐标轴对齐)。<br>

# 2.热扰动 <br>
1.实现热扰动效果(包含了对不透明物体和透明物体的两种扰动)。<br>
2.给urp管线添加一个包含透明物体渲染结果的颜色缓冲(原始的_CameraOpaqueTexture只包含不透明物体
的渲染结果)，并给管线根据所需颜色缓冲的情况，添加了两个LightMode用于控制。	<br>

# 3.重生 <br>
从官方3D Game Kit中借鉴来的角色重生效果。<br>

# 4.消融 <br>
包含定向消融和中心扩散消融两种效果。 <br>

# 5.相机近裁剪扰动消失 <br>
相机穿模时，加个渐渐消失的效果。 <br>